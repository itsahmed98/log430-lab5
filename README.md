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

-   ASP.NET Core (.NET 8)
-   Docker / Docker Compose
-   PostgreSQL
-   Kong API Gateway
-   Prometheus + Grafana
-   PowerShell (pour configuration automatique de Kong)
-   HTTP Client + Swagger (OpenAPI)

---

## Environnement du production et developpement

Les environnements sont gérés via appsettings.Development.json (local direct) et appsettings.Production.json (via Kong).

Chaque microservice utilise son propre schéma de base de données PostgreSQL.

Swagger est activé dans tous les services pour la documentation automatique.

---

## Architecture des microservices

-   `MagasinCentral` : application client MVC Razor
-   `CatalogueMcService` : gestion des produits (ajouts, modification, recherche)
-   `InventaireMcService` : gestion du stock central et local, et réapprovisionnements
-   `VenteMcService` : Enregistrement des ventes en magasin (POS) et ventes en ligne via commandes validées (ECommerce)
-   `AdministrationMcService` : Générer les rapports consolidés des ventes et les performances des magasins
-   `ECommerceMcService` : Gestion du parcours client (Création de compte, panier, commandes en ligne)
-   `Kong` : passerelle API avec routage, sécurité, logging, load balancing
-   `PostgreSQL` : une base de données par microservice
-   `Prometheus / Grafana` : monitoring

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

docker-compose down -v
docker-compose build
docker-compose up -d
.\configure-kong.ps1

---

## Accès aux services

| Composant           | URL                                                                   |
| ------------------- | --------------------------------------------------------------------- |
| Application MVC     | [http://localhost:8080](http://localhost:8080)                        |
| Swagger (client)    | [http://localhost:8080/swagger](http://localhost:8080/swagger)        |
| CatalogueMcService  | [http://http://localhost:5001/swagger](http://localhost:5001/swagger) |
| InventaireMcService | [http://http://localhost:5002/swagger](http://localhost:5002/swagger) |
| VenteMcService      | [http://http://localhost:5003/swagger](http://localhost:5003/swagger) |
| AdminMcService      | [http://http://localhost:5004/swagger](http://localhost:5004/swagger) |
| ECommerceMcService  | [http://http://localhost:5005/swagger](http://localhost:5005/swagger) |
| Kong (API Gateway)  | [http://localhost:8000](http://localhost:8000)                        |
| Kong Admin          | [http://localhost:8001](http://localhost:8001)                        |
| Prometheus          | [http://localhost:9090](http://localhost:9090)                        |
| Grafana             | [http://localhost:3000](http://localhost:3000) (admin/admin)          |

---

## Configuration de Kong

Un script PowerShell configure-kong.ps1 permet de :

-   Créer les services et routes dynamiques pour chaque microservice (/catalogue, /vente, etc.)
-   Ajouter des plugins (clés API, logging, etc.)

pour l'éxécuter, allez dans le dossier contenant le script et faites ce commande:

-   .\configure-kong.ps1

---
