import os
os.environ["PHONEMIZER_ESPEAK_LIBRARY"] = r"C:\Program Files\eSpeak NG\libespeak-ng.dll"
os.environ["PHONEMIZER_ESPEAK_PATH"] = r"C:\Program Files\eSpeak NG"

from flask import Flask, request, send_file
from kokoro import KPipeline
import io
import soundfile as sf

app = Flask(__name__)

# Le pipeline cherchera automatiquement 'kokoro-v1_0.pth' et 'config.json' dans ton dossier.
pipeline = KPipeline(lang_code='f') 

@app.route('/tts')
def tts():
    text = request.args.get('text')
    voice = request.args.get('voice', 'ff_siwis')  # Voix par défaut
    
    if not text:
        return "Texte manquant", 400

    generator = pipeline(text, voice=voice, speed=1)
    
    # On récupère le premier segment audio généré
    for _, _, audio in generator:
        buffer = io.BytesIO()
        # Les modèles .pth fonctionnent généralement en 24000Hz
        sf.write(buffer, audio, 24000, format='WAV')
        buffer.seek(0)
        return send_file(buffer, mimetype='audio/wav')

if __name__ == '__main__':
    print("Le serveur de Pierre est prêt sur le port 8888 !")
    app.run(host='0.0.0.0', port=8888)