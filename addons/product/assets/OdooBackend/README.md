# OdooBackend - Instructions de démarrage

## Prérequis

- .NET 8.0 SDK installé
- Odoo en cours d'exécution (via Docker Compose ou autre)

## Démarrage du serveur backend

Pour démarrer le serveur API backend sur le port 5500:

```bash
cd addons/product/assets/OdooBackend
dotnet run
```

Le serveur démarrera sur `http://localhost:5500`

## Accès à l'interface utilisateur

Une fois le serveur backend démarré, ouvrez le fichier HTML dans votre navigateur:

- `wwwroot/home/home.html` - Page de configuration et chargement des produits

## Configuration

### Backend API
Le serveur est configuré pour écouter sur le port 5500 (voir `Properties/launchSettings.json`)

### Frontend
Le frontend est configuré pour se connecter à `http://localhost:5500/api/products/loads` (voir `wwwroot/home/script.js`)

### Odoo
Assurez-vous qu'Odoo est accessible via Docker Compose:
```bash
# Depuis la racine du projet
docker-compose up -d
```

Odoo sera accessible sur `http://localhost:8069`

## Résolution des problèmes

### Erreur de connexion
Si vous obtenez une erreur "Impossible de joindre le serveur backend":
1. Vérifiez que le serveur backend est démarré avec `dotnet run`
2. Vérifiez que le port 5500 n'est pas utilisé par une autre application
3. Vérifiez les logs de la console du serveur backend

### Erreur d'authentification Odoo
1. Vérifiez que les conteneurs Docker sont en cours d'exécution
2. Vérifiez les identifiants de connexion dans le formulaire HTML
3. Vérifiez les logs Odoo pour plus de détails

### Problèmes CORS
Le backend est configuré pour accepter toutes les origines (CORS ouvert). Si vous rencontrez des problèmes CORS, vérifiez la configuration dans `Program.cs`.
