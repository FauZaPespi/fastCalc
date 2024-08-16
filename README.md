
<br/>
<div align="center">
<a href="https://github.com/ShaanCoding/ReadME-Generator">
<img src="https://cdn.discordapp.com/attachments/1180334194329649212/1273971201919815791/icons8-erreur-de-calcul-64.png?ex=66c08da3&is=66bf3c23&hm=11395c847b0f2d45aafd6b67a62e0d8a18a74ff142fddc595b381ac4189309d3&" alt="Logo" width="80" height="80">
</a>
<h3 align="center">FastCalc</h3>
<p align="center">
Calculator that works with sharing

<br/>
<br/>
<a href="https://discord.com/users/1172167256470474775">Contact me</a>  


</p>
</div>

## About The Project

![ta faim ?](https://c4.wallpaperflare.com/wallpaper/439/731/824/anime-girls-hatsune-miku-moon-night-wallpaper-preview.jpg)

#### Contexte du Projet
Le projet consiste à développer un système client-serveur en C# permettant aux clients d'effectuer des calculs complexes. Le serveur gère la distribution des calculs aux clients disponibles, stocke les résultats des calculs dans un fichier JSON, et renvoie les résultats aux clients qui ont demandé les calculs. Les clients peuvent également signaler que certains calculs sont trop complexes à gérer et les renvoyer au serveur pour redistribution.

##### Architecture du Système

#### Serveur (Server) :

##### Rôle : Le serveur reçoit des demandes de calculs des clients, attribue ces demandes à d'autres clients pour traitement, stocke les résultats dans un fichier JSON, et renvoie les résultats aux clients demandeurs.
Fonctionnalités :
Gestion des connexions : Établit des connexions avec plusieurs clients et gère les communications.
Répartition des calculs : Reçoit les demandes de calculs des clients et les distribue à d'autres clients disponibles pour traitement.
Stockage des résultats : Vérifie si un calcul a déjà été effectué en consultant le fichier JSON et enregistre les nouveaux résultats.
Gestion des erreurs : Gère les cas où un client signale qu'un calcul est trop complexe pour lui.

#### Client (Client) :

##### Rôle : Les clients envoient des demandes de calcul au serveur, reçoivent des calculs à effectuer, réalisent les calculs, et envoient les résultats au serveur.
Fonctionnalités :
Envoi de calculs : Permet à l'utilisateur de saisir des calculs à envoyer au serveur.
Réception et traitement des calculs : Reçoit des calculs du serveur, les effectue, et renvoie les résultats.
Gestion des calculs complexes : Signale les calculs trop complexes et les renvoie au serveur pour une nouvelle attribution.
Fonctionnalités Clés
#### Serveur

##### Démarrage et Gestion des Clients : Le serveur démarre, accepte les connexions des clients, et maintient une liste de clients connectés.
Répartition des Calculs : Lorsqu'une demande de calcul est reçue, le serveur vérifie si le calcul a déjà été effectué. Si non, il l'attribue à un autre client disponible.
Stockage des Résultats : Les résultats des calculs sont stockés dans un fichier JSON avec un identifiant unique pour chaque calcul. Cela permet de vérifier si un calcul a déjà été réalisé.
Gestion des Exceptions : Le serveur gère les déconnexions des clients et les erreurs de transmission.
#### Client

##### Envoi de Calculs : L'utilisateur peut entrer des expressions mathématiques à envoyer au serveur pour calcul.
Réception de Calculs : Les clients reçoivent des calculs du serveur, effectuent les calculs et renvoient les résultats.
Gestion des Calculs Trop Complexes : Si un calcul est trop complexe pour être effectué, le client le signale au serveur pour redistribution.
Exemples d'Utilisation
##### Lancement du Serveur

Le serveur est démarré et écoute sur le port 8888 pour les connexions des clients.
##### Connexion des Clients

Trois clients se connectent au serveur. Le serveur affiche les connexions et leur attribue des identifiants.
##### Envoi et Traitement des Calculs

Un client envoie un calcul comme 3+4 au serveur.
Le serveur vérifie si ce calcul a déjà été effectué. Si non, il l'attribue à un autre client.
Le client receveur effectue le calcul et renvoie le résultat au serveur.
Le serveur stocke le résultat dans le fichier JSON et le renvoie au client demandeur.
##### Gestion des Calculs Trop Complexes

Si un client reçoit un calcul trop complexe (comme un calcul avec de très grands nombres ou des expressions complexes), il signale au serveur que le calcul est trop difficile.
Le serveur redistribue ce calcul à un autre client disponible.
##### Gestion des Erreurs
Déconnexion des Clients : Le serveur gère les déconnexions des clients en nettoyant les connexions fermées et en évitant les exceptions non gérées.
Erreurs de Transmission : Les exceptions liées à la transmission des données (comme les erreurs de lecture/écriture) sont capturées et gérées.
### Built With

J'ai seulement utiliser le C#...

- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
## Usage

Il faut d'abord lancer le serveur puis au moins 2 clients sinon ce sera de la merde
## Roadmap

- [x] Faire le projet

See the [open issues](https://discord.com/users/1172167256470474775) for a full list of proposed features (and known issues).
## Contributing

English:
If you want to contribute, join the Palms server: [https://discord.gg/gRQZXHwMsZ](https://discord.gg/gRQZXHwMsZ)

Français :
Si tu veux participer à d'autres projets ou améliorer celui-ci, rejoins le serveur Discord des Palms : [https://discord.gg/gRQZXHwMsZ](https://discord.gg/gRQZXHwMsZ)
## Contact

FauZaPespi - [@FauZaPespi](https://discord.com/users/1172167256470474775) - [https://fauza.fr](https://fauza.fr)

Project Link: [https://github.com/FauZaPespi/fastCalc](https://github.com/FauZaPespi/fastCalc)
