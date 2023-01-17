FROM python:latest

LABEL author="Arnór Moncada"

COPY . /app

WORKDIR /app

#get the luhn algorithm from the requirements.txt file
RUN pip3 install -r requirements.txt


ENTRYPOINT ["python3","-u", "email_service.py"]