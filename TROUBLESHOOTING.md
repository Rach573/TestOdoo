# Guide de dépannage - Connexion Odoo

## Problème : "Connection refused (localhost:8069)"

### Étape 1 : Vérifier qu'Odoo est démarré

```bash
docker ps
```

Vous devriez voir deux conteneurs en cours d'exécution :
- Un conteneur Odoo (port 8069)
- Un conteneur PostgreSQL

Si les conteneurs ne sont pas visibles :
```bash
docker-compose up -d
```

### Étape 2 : Vérifier qu'Odoo répond

Ouvrez votre navigateur et allez sur : **http://localhost:8069**

Vous devriez voir la page d'accueil d'Odoo.

### Étape 3 : Créer/Configurer la base de données

**IMPORTANT:** La première fois, vous devez créer une base de données Odoo.

1. Allez sur http://localhost:8069
2. Si aucune base de données n'existe, vous verrez un formulaire de création
3. Créez une base de données avec ces informations :
   - **Nom de la base :** `odoo` (ou le nom que vous préférez)
   - **Email :** admin (ou votre email)
   - **Mot de passe :** choisissez un mot de passe
   - **Langue :** Français
   - **Pays :** France

### Étape 4 : Utiliser les bons identifiants dans l'application

Dans le formulaire HTML, utilisez :

| Champ | Valeur |
|-------|--------|
| **URL Odoo** | `http://localhost:8069` |
| **Base de données** | Le nom que vous avez créé (ex: `odoo`) |
| **Utilisateur** | L'email que vous avez utilisé (ex: `admin` ou votre email) |
| **Mot de passe** | Le mot de passe que vous avez défini lors de la création |

### Étape 5 : Problèmes courants

#### Erreur : "Database doesn't exist"
- Vérifiez que vous avez bien créé la base de données via l'interface web d'Odoo
- Vérifiez que le nom de la base correspond exactement

#### Erreur : "Access Denied" ou "Invalid credentials"
- Vérifiez votre nom d'utilisateur (email) et mot de passe
- Assurez-vous d'utiliser les identifiants que vous avez créés dans Odoo

#### Erreur : "Connection refused"
- Vérifiez que les conteneurs Docker sont en cours d'exécution avec `docker ps`
- Vérifiez que le port 8069 est accessible : `curl http://localhost:8069`
- Redémarrez les conteneurs : `docker-compose restart`

### Vérification des logs

Pour voir les logs d'Odoo :
```bash
docker-compose logs web
```

Pour voir les logs de PostgreSQL :
```bash
docker-compose logs mydb
```

### Redémarrage complet

Si rien ne fonctionne :
```bash
# Arrêter tout
docker-compose down

# Redémarrer
docker-compose up -d

# Attendre 30 secondes que tout démarre
sleep 30

# Vérifier l'état
docker ps
```

## Configuration Docker Compose

Selon votre `compose.yml`, les paramètres PostgreSQL sont :
- **Database :** `postgres` (base système)
- **User :** `odoo`
- **Password :** `myodoo`

Mais vous devez créer votre propre base de données Odoo via l'interface web (étape 3).
