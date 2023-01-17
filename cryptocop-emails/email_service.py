import pika
import requests
import json
from retry import retry
import os


def is_docker():
    """Check if the application is running in a docker container"""
    path = '/proc/self/cgroup'
    return (
        os.path.exists('/.dockerenv') or
        os.path.isfile(path) and any('docker' in line for line in open(path))
    )


@retry(pika.exceptions.AMQPConnectionError, delay=5, jitter=(1, 3))
def get_connection(connection_string):
    connection = pika.BlockingConnection(
        pika.ConnectionParameters(connection_string))
    return connection


# connect to rabbitmq if in docker container else use localhost
if is_docker():
    connecion_string = 'rabbitmq'
else:
    connecion_string = 'localhost'

connection = get_connection(connecion_string)
channel = connection.channel()
exchange_name = 'order_exchange'
create_order_routing_key = 'create-order'
email_queue_name = 'email_queue'
email_template = '<h2>Thank you for ordering!</h2><p>We hope you will enjoy our lovely product and don\'t hesitate to contact us if there are any questions.</p><table><thead><tr style="background-color: rgba(155, 155, 155, .2)"><th>Name of Customer</th><th>Address</th><th>Country</th><th>Date of order</th></tr></thead><tbody>%s</tbody></table>'

# Declare the exchange, if it doesn't exist
channel.exchange_declare(exchange=exchange_name,
                         exchange_type='topic', durable=True)
# Declare the queue, if it doesn't exist
channel.queue_declare(queue=email_queue_name, durable=True)
# Bind the queue to a specific exchange with a routing key
channel.queue_bind(exchange=exchange_name, queue=email_queue_name,
                   routing_key=create_order_routing_key)


def send_simple_message(to, subject, body):
    return requests.post(
        "https://api.mailgun.net/v3/sandboxbd8d82d7f021437ba1f0a70131712472.mailgun.org/messages",
        auth=("api", "677f734df29a5ff60a15cf23eec68c5a-b0ed5083-9205463b"),
        data={"from": "Mailgun Sandbox <postmaster@sandboxbd8d82d7f021437ba1f0a70131712472.mailgun.org>",
              "to": to,
              "subject": subject,
              "html": body})


def send_order_email(ch, method, properties, data):
    parsed_msg = json.loads(data)
    print(parsed_msg)
    # create the address from the data
    address = parsed_msg['streetName'] + ' ' + \
        parsed_msg['houseNumber'] + ', ' + \
        parsed_msg['city'] + ', ' + parsed_msg['zipCode']

    # create the order items list
    order_items = ''
    for item in parsed_msg['orderItems']:
        order_item = ''.join(['<tr><td>', item['productIdentifier'], '</td><td>',
                              str(item['quantity']), '</td><td>', str(item['unitPrice']), '</td><td>', str(item['totalPrice']), '</td></tr>'])
        order_items += order_item

    items_html = ''.join(['<tr><td>%(fullName)s</td><td>%(address)s</td><td>%(country)s</td><td>%(date)s</td></tr>' %
                          {'fullName': parsed_msg['fullName'],
                           'address':  address, 'country': parsed_msg['country'], 'date': parsed_msg['orderDate']}])
    representation = email_template % items_html
    order_item_table = '<table><thead><tr style="background-color: rgba(155, 155, 155, .2)"><th>Product</th><th>Quantity</th><th>Unit price</th><th>Item total price</th></tr></thead><tbody>%s</tbody></table>' % order_items
    representation += '<br>' + order_item_table

    total_price_table = '<table><thead><tr style="background-color: rgba(155, 155, 155, .2)"><th>Total price</th></tr></thead><tbody>%s</tbody></table>' % '<tr><td>' + str(
        parsed_msg['totalPrice']) + '</td></tr>'
    representation += '<br>' + total_price_table
    response = send_simple_message(
        parsed_msg['email'], 'Successful order!', representation)


print(' [*] Waiting for messages. To exit press CTRL+C')
channel.basic_consume(on_message_callback=send_order_email,
                      queue=email_queue_name,
                      auto_ack=True)

channel.start_consuming()
connection.close()
