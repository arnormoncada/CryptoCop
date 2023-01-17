import pika
import json
from cctek import luhn_checker
import os
from retry import retry


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
email_queue_name = 'payment_queue'

# Declare the exchange, if it doesn't exist
channel.exchange_declare(exchange=exchange_name,
                         exchange_type='topic', durable=True)
# Declare the queue, if it doesn't exist
channel.queue_declare(queue=email_queue_name, durable=True)
# Bind the queue to a specific exchange with a routing key
channel.queue_bind(exchange=exchange_name, queue=email_queue_name,
                   routing_key=create_order_routing_key)


def validate_payment_card(ch, method, properties, data):
    # get data from message
    parsed_msg = json.loads(data)
    # extract the card number from json
    card_json = parsed_msg['creditCard']

    # use luhn algorithm to validate card number
    try:
        is_valid = luhn_checker(card_json)
        if is_valid:
            print('Card is valid')
        else:
            print('Card is invalid')
    except (ValueError):
        # catch the value error if the card number is not a number
        print("Invalid payment card number")


print(' [*] Waiting for messages. To exit press CTRL+C')
channel.basic_consume(on_message_callback=validate_payment_card,
                      queue=email_queue_name,
                      auto_ack=True)

channel.start_consuming()
connection.close()
