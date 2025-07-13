# Magasin Central - Architecture Microservices avec Kong

Ce projet est une application de gestion de magasins développée selon une architecture microservices, orchestrée via Docker Compose, avec **Kong API Gateway** pour la gestion centralisée du routage, de la sécurité, et du load balancing.

---

## Structure du projet

log430-lab5/
│
├── MagasinCentral/ → Application MVC Razor
├── CatalogueMcService/ → Microservice catalogue
├── VenteMcService/ → Microservice ventes
├── InventaireMcService/ → Microservice inventaire
├── AdministrationMcService/ → Microservice rapports/performance
├── ECommerceMcService/ → Microservice e-commerce
│
├── docker-compose.yml → Démarrage des services
├── configure-kong.ps1 → Script de configuration de Kong
└── README.md → Contient les instructions du démarrage du projet

## Technologies utilisées

- ASP.NET Core (.NET 8)
- Docker / Docker Compose
- PostgreSQL
- Kong API Gateway
- Prometheus + Grafana
- PowerShell (pour configuration automatique de Kong)
- HTTP Client + Swagger (OpenAPI)

---

## Environnement du production et developpement

Les environnements sont gérés via appsettings.Development.json (local direct) et appsettings.Production.json (via Kong).

Chaque microservice utilise son propre schéma de base de données PostgreSQL.

Swagger est activé dans tous les services pour la documentation automatique.

---

## Architecture des microservices

- `MagasinCentral` : application client MVC Razor
- `CatalogueMcService` : gestion des produits (ajouts, modification, recherche)
- `InventaireMcService` : gestion du stock central et local, et réapprovisionnements
- `VenteMcService` : Enregistrement des ventes en magasin (POS) et ventes en ligne via commandes validées (ECommerce)
- `AdministrationMcService` : Générer les rapports consolidés des ventes et les performances des magasins
- `ECommerceMcService` : Gestion du parcours client (Création de compte, panier, commandes en ligne)
- `Kong` : passerelle API avec routage, sécurité, logging, load balancing
- `PostgreSQL` : une base de données par microservice
- `Prometheus / Grafana` : monitoring

---

## Démarrage en production

### 1. Cloner le projet

```bash
git clone https://github.com/itsahmed98/log430-lab5.git
cd log430-lab5
```

### 2. Construire les images Docker

docker-compose build

### 3. Lancer tous les conteneurs

docker-compose up -d

---

## Redémarrage rapide

Si vous voulez redemmarer tout de zéro :

docker-compose down -v (attention! cela va enlever les dashboards grafana deja faits)
docker-compose build
docker-compose up -d
.\configure-kong.ps1

---

## Accès aux services

| Composant            | URL                                                                                                                       |
| -------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Application MVC      | [http://localhost:8080](http://localhost:8080)                                                                            |
| Swagger (API client) | [http://localhost:8080/swagger](http://localhost:8080/swagger)                                                            |
| CatalogueMcService   | [http://http://localhost:5001/swagger](http://localhost:5001/swagger)                                                     |
| InventaireMcService  | [http://http://localhost:5002/swagger](http://localhost:5002/swagger)                                                     |
| VenteMcService       | [http://http://localhost:5003/swagger](http://localhost:5003/swagger)                                                     |
| AdminMcService       | [http://http://localhost:5004/swagger](http://localhost:5004/swagger)                                                     |
| ECommerceMcService   | [http://http://localhost:5005/swagger](http://localhost:5005/swagger)                                                     |
| Kong (API Gateway)   | [http://localhost:8000](http://localhost:8000)                                                                            |
| Kong Admin           | [http://localhost:8001](http://localhost:8001)                                                                            |
| Prometheus           | [http://localhost:9090](http://localhost:9090)                                                                            |
| Grafana              | [http://localhost:3000](http://localhost:3000) login: admin et password: admin (peut être changer après le premier login) |
| Grafana dashboards   | [http://localhost:3000](http://localhost:3000/dashboards)                                                                 |

---

## Configuration de Kong

À retenir! le script .\configure-kong.ps1 contient dèja tout les configurations nécéssaire pour kong et les plugins. il suffit juste de l'éxécuter.
Par contre si vous souhaitez ajouter manuellement des configurations, voici comment faire:

### Demarrage

Un script PowerShell configure-kong.ps1 permet de :

- Créer les services et routes dynamiques pour chaque microservice (/catalogue, /vente, etc.)
- Ajouter des plugins (clés API, logging, etc.)

pour l'éxécuter, allez dans le dossier contenant le script et faites ce commande:

- .\configure-kong.ps1

---

### LoadBalancer

Le script declare un upstream pour 2 instances du catalogueMcService au nom: catalogue-api-1 et -2
@{ name = "catalogue-service"; url = "http://catalogue-upstream"; path = "/catalogue"; strip_path = $true; upstream = $true; targets = @("catalogue-api-1:80", "catalogue-api-2:80") }

Ensuite vous pouvez aller au (GET http://localhost:8001/upstreams/catalogue-upstream/targets) pour les details

Pour ajouter un target: POST http://localhost:8001/upstreams/catalogue-upstream/targets avec body en format x-www-form-urlencoded:

- target: catalogue-api-2:80
- weight: 2

Pour supprimer un target: DELETE http://localhost:8001/upstreams/catalogue-upstream/targets/catalogue-api-2:80

### Ajout des clés API

Dans le script, le microservice ECommerceMcService est protéger et il faut une clé api pour accèder ses endpoints. Après l'éxécution du script vous aurez un clé API, il faut ensuite ajouter un header au nom "apikey" avec la valeur de la clé générée.

**Pour une raison d'absence d'authentification et gestion des clés utilisateurs à partir de client-app, j'ai commenté les sections dans le script qui configure la protection des endpoints. Il suffit de de-commenter ces sections et tester les endpoints de ecommerce avec Postman et pas à partir du client app.**

### Sécurité et gestion des accès (CORS et journaux d'accès) - Plugins

Pour voir les plugins tels que CORS et journaux d'accès, faites une requête vers : GET http://localhost:8001/services/{nom-service}/plugins
Vous allez trouver les plugins sous: "name": "file-log" et "name": "CORS"

**Ajouter PLUGIN des règles de CORS à un service via Kong Admin API**

POST http://localhost:8001/services/{nom-service}/plugins

Méthode 1 – x-www-form-urlencoded (dans Postman) :

name: cors
config.origins: \*
config.methods: GET, POST, PUT, DELETE, OPTIONS
config.headers: Accept, Authorization, Content-Type
config.exposed_headers: X-Custom-Header
config.credentials: true
config.max_age: 3600

Méthode 2 – JSON (si tu choisis raw > JSON dans Postman) :

{
"name": "cors",
"config": {
"origins": ["*"],
"methods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
"headers": ["Accept", "Authorization", "Content-Type"],
"exposed_headers": ["X-Custom-Header"],
"credentials": true,
"max_age": 3600
}
}

**Ajouter PLUGIN Journaux d'accès**

Pré-requis: Avoir configuré un volume sur le container Kong pour accéder aux logs localement

- kong:
  ...
  volumes: - ./kong-logs:/var/log/kong

curl ou Postman:
curl -X POST http://localhost:8001/services/catalogue-service/plugins \
 --data "name=file-log" \
 --data "config.path=/var/log/kong/access.log"

Une fois configuré, les logs se trouvent dans ./kong-logs/access.log
