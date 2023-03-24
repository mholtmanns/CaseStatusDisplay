from websocket_server import WebsocketServer

def new_client(client, server):
    print("New client connected and was given id %d" % client['id'])

def message_received(client, server, message):
    server.send_message_to_all(message)
    print("Message received: %s" % message)

server = WebsocketServer(port = 8080)
server.set_fn_new_client(new_client)
server.set_fn_message_received(message_received)
server.run_forever()