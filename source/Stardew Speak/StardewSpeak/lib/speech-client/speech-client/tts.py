import server

def speak(text: str):
    server.send_message("TTS_SPEAK", {'text': text})