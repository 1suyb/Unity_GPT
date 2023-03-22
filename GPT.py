import socket,threading
import os
import openai

def binder(client_socket, addr) :
    try : 
        while True:
            data = client_socket.recv(4)
            length = int.from_bytes(data,"big")
            if length == 0 :
                continue
            data = client_socket.recv(length)
            msg = data.decode()
            print(type(msg))
            msg = diaGen(msg)
            print(msg)
            data = msg.encode()
            length = len(data)
            client_socket.sendall(length.to_bytes(4,byteorder='big'))
            client_socket.sendall(data)
            
    except Exception as e :
        print(f"except : {0}, error : {1}", addr, e)
    finally :
        client_socket.close()

def diaGen(message) :
    print("diaGen")
    completion = openai.ChatCompletion.create(
        model="gpt-3.5-turbo",
        messages=[
            {"role": "user", "content": message}
        ]
    )
    print("completion")
    response = completion.choices[0].message['content']
    return response




openai.api_key = os.getenv('GPT_API_KEY')

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind(('',9999))
server_socket.listen()

try : 
    print("yes")
    while True :
        client_socket, addr = server_socket.accept()
        if client_socket is not None :
            break
    binder(client_socket, addr)
except :
    print("server")
finally :
    server_socket.close()
    os.system("pause")
    print("abc{}",client_socket)
