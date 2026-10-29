# Dumb Stardew 

### Description du projet 

Ce projet Dumb Stardew est une expérience de réalité virtuelle 3D développée sous Unity. Inspiré par l'univers de Stardew Valley, il permet à l'utilisateur d'explorer les champs d'une ferme. Les cultures et l'environnement sont différents selon les saisons. L'objectif est de découvrir de nouvelles informations, fournies par un agent LLM, sur les végétaux du jeu et cela de manière ludique. 
Réalisé à 3 : Amélie L, Riane Mbodji et Nolwenn T.
### Fonctionnalités clés 

* Cycle des saisons dynamique : Visualisation des quatre saisons (Printemps, Été, Automne, Hiver) et de leurs cultures au sein d'une même scène en fonction de l'orientation de l'utilisateur
* Système d'agrippement : Interaction directe avec les fruits et légumes de saison
* Guide : Un fermier est présent pour vous donner des informations sur les objets que vous lui apportez 

### Éléments multimodaux 

Le projet intègre : 
* Modèles 3D : Large collection d'assets (ferme, outils, animaux et végétaux)
* Textes : Panneau d'affichage (Canvas) dynamique présentant les données de l'agent LLM 
* Audios : Système de Text-to-Speech via le serveur Kokoro pour une lecture à haute voix des informations

### Système VR 
#### Navigation

* Déplacement et Rotation : Déplacement fluides basés sur le template VR officiel d'Unity
* Mouvement de tête : Intégration du suivi du casque dans la scène

#### Interactions

* Système de Grab : Utilisation du XR Grab Interactor sur les deux contrôleurs pour saisir les objets
* Physique : Configuration des RigidBody et des Colliders sur les objets pour une manipulation réaliste

#### Affordance 

Les contrôleurs vibrent si leur pointeur est sur un objet qu'il est possible d'attraper. De plus, il y a un retour sonore lors de la réponse du fermier.  

### Agent LLM 
#### Que fait réellement l'agent IA pour l'utilisateur final ? 

L'agent, ici le fermier, agit comme un guide pour l'utilisateur. L'utilisateur présente un objet, dès lors l'agent génère une explication simplifiée, contenant la saison et des conseils pour le joueur, affichée visuellement et lue vocalement.

Technologies : 
* Intégration d'un LLM via Groq 
* Serveur local avec Kokoro-82M pour le traitement vocal

### Installation et Utilisation 
#### Pré-requis

* Casque VR (Meta Quest testé).
* Python 
* Unity 2022.3+ avec support Android/XR

#### Lancement du serveur vocal (Kokoro)

1. Aller dans le dossier Kokoro Server

2. Installer **toutes** les dépendances : pip install torch kokoro soundfile flask

3. Lancer le serveur : python kokoro_server.py

4. **Important** : Autoriser Python à utiliser le réseau privé dans les configurations Windows 

***Attention*** 
Il faut lancer le serveur sur le même wifi que le casque VR. 
De plus il peut y avoir des problèmes pour démarrer le serveur, il faut installer les bonnes dépendances. 

#### Configuration Unity 

1. Ouvrir le projet sur Unity 
2. Ajouter les assets à l'espace de travail
3. Lancer le serveur Kokoro puis localiser l'objet NPC AGENT dans la hiérarchie et renseigner l'ip du serveur dans le champ *Kokoro Server URL* 
4. Faire un Build & Run (Fichier > Build & Run) en ayant le casque VR branché 

### Description des scripts
#### Serveur Kokoro
* kokoro_server.py : Script Python pour le serveur de synthèse vocale Kokoro. Il reçoit des requêtes POST avec du texte, génère un fichier audio et le renvoie au client.
* config.json : Fichier de configuration pour le serveur Kokoro, spécifiant les paramètres du modèle de synthèse vocale. Il faut le récupérer depuis le dépôt Kokoro-82M et le placer dans le même dossier que le script kokoro_server.py.
* kokoro-v1_0.pth : Modèle de synthèse vocale pré-entraîné pour Kokoro. Il doit être téléchargé depuis le dépôt Kokoro-82M et placé dans le même dossier que le script kokoro_server.py.

#### Unity
* KokoroTTS.cs : Script C# pour Unity qui gère la communication avec le serveur Kokoro. Il envoie des requêtes POST avec du texte et reçoit les fichiers audio générés.
* NPCAgent.cs : Script C# pour Unity qui gère le comportement de l'agent NPC. Il interagit avec les objets saisis par l'utilisateur, génère des réponses textuelles et utilise KokoroTTS pour la synthèse vocale.
* NPCProximityTalker.cs : Script C# pour Unity qui gère la détection de proximité entre l'utilisateur et l'agent NPC. Il déclenche des interactions lorsque l'utilisateur est à proximité de l'agent.
* LLMClient.cs : Script C# pour Unity qui gère la communication avec le LLM (via Groq). Il envoie des requêtes avec des informations sur les objets saisis et reçoit des réponses textuelles à afficher et à vocaliser.
* NPCTestDriver.cs : Script C# pour Unity utilisé pour les tests. Il permet de simuler des interactions avec l'agent NPC sans avoir besoin de la VR, en appelant directement les méthodes de NPCAgent pour générer des réponses textuelles et vocales.
* FreeTTS.cs : Script C# pour Unity qui gère la synthèse vocale de base en utilisant une API de Text-to-Speech Google. Il peut être utilisé comme alternative à Kokoro pour les tests ou si le serveur Kokoro n'est pas disponible. Pour l'utiliser, il suffit de remplacer les appels à KokoroTTS par des appels à FreeTTS dans le script NPCAgent.cs, et modifier la variable présente dans le fichier NPCAgent.cs pour choisir entre les deux systèmes de synthèse vocale.

### Assets 

Le projet utilise une sélection d'assets pour l'ambiance, les végétaux et l'agent LLM. 

Environnement : 
* Low Poly Farm Pack Lite : https://assetstore.unity.com/packages/3d/environments/industrial/low-poly-farm-pack-lite-188100
*  Fantasy Landscape : https://assetstore.unity.com/packages/3d/environments/fantasy-landscape-103573 
* Farm Ranch low poly Pack : https://assetstore.unity.com/packages/3d/props/pandazole-farm-ranch-low-poly-pack-206756 
*  Animated Low Poly 3D Models : https://assetstore.unity.com/packages/3d/characters/animals/animals-free-animated-low-poly-3d-models-260727 


Végétaux : 
* Food Fruits : https://assetstore.unity.com/packages/3d/props/food/food-fruits-306724 
* Casual Vegetable Pack : https://assetstore.unity.com/packages/3d/props/food/casual-vegetable-pack-created-with-fastmesh-asset-293783  
* Low Poly Trees : https://assetstore.unity.com/packages/3d/vegetation/trees/low-poly-trees-nature-pack-70-props-325300 

Personnages : 
* Stylized NPC Peasant : https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/stylized-npc-peasant-nolant-demo-252440

Outils de jardinage : 
* Toontastic Farm Tools : https://assetstore.unity.com/packages/3d/props/tools/toontastic-farm-tools-265686 
