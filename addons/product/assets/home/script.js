// =========================
// Variables globales session Odoo
// =========================
let odooSessionId = null;
let odooUid = null;

// =========================
// Récupérer la configuration depuis le formulaire
// =========================
function getConfig() {
  return {
    url: document.getElementById('url').value,
    db: document.getElementById('db').value,
    user: document.getElementById('user').value,
    password: document.getElementById('password').value
  };
}

// =========================
// Authentification Odoo (JSON-RPC 2.0)
// =========================
async function authenticate() {
  const cfg = getConfig();
  const status = document.getElementById('status');

  // On utilise l'URL telle que saisie dans le champ
  const endpoint = cfg.url + "/web/session/authenticate";

  // Payload JSON-RPC 2.0
  const payload = {
    jsonrpc: "2.0",
    method: "call",
    params: {
      db: cfg.db,
      login: cfg.user,
      password: cfg.password
    },
    id: Math.floor(Math.random() * 100000) // id aléatoire
  };

  try {
    status.textContent = "Connexion à Odoo en cours...";

    const response = await fetch(endpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Accept": "application/json"
      },
      body: JSON.stringify(payload),
      credentials: "include"
    });

    if (!response.ok) {
      throw new Error("Erreur réseau HTTP : " + response.status);
    }

    const data = await response.json();

    // Erreur côté Odoo (login / password / db etc.)
    if (data.error) {
      const msg =
        (data.error.data && data.error.data.message) ||
        data.error.message ||
        "Erreur d'authentification";
      throw new Error(msg);
    }

    if (!data.result || !data.result.session_id || data.result.uid == null) {
      throw new Error("Réponse Odoo invalide : session_id / uid manquants");
    }

    // Stockage global
    odooSessionId = data.result.session_id;
    odooUid = data.result.uid;

    console.log("Authentification réussie.", {
      session_id: odooSessionId,
      uid: odooUid
    });

    status.textContent = `Authentification réussie (uid: ${odooUid}).`;
    return true;
  } catch (err) {
    console.error("Erreur d'authentification :", err);
    status.textContent = "Erreur d'authentification : " + err.message;
    odooSessionId = null;
    odooUid = null;
    return false;
  }
}

async function callOdoo(method, model, args = [], kwargs = {}) {
 const cfg = getConfig();
 const endpoint = cfg.url + "/web/session/call_kw";
{
  const payload = {
  jsonrpc: '2.0',
  method: 'call',
  model: 'product.template',
  method: search_read,
  args: args,
  kwargs: kwargs
  }   

  const endpoint = cfg.url + "/web/session/call_kw";
}
try {
    const response = await fetch(endpoint, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Accept": "application/json"
      },
      body: JSON.stringify(payload),
      credentials: "include" // on garde le cookie de session envoyé par authenticate()
    });

    if (!response.ok) {
      throw new Error("Erreur réseau HTTP : " + response.status);
    }

    const data = await response.json();

    if (data.error) {
      const msg =
        (data.error.data && data.error.data.message) ||
        data.error.message ||
        "Erreur lors de l'appel Odoo";
      throw new Error(msg);
    }

    // data.result contient le "vrai" résultat de l'appel
    return data.result;

  } catch (err) {
    console.error("Erreur callOdoo:", err);
    // on remonte l'erreur au code appelant
    throw err;
  }
}
// =========================
// Données de démonstration : produits factices
// =========================
const fakeProducts = [
  {
    id: 1,
    name: "Chambre double",
    default_code: "ROOM-D-01",
    list_price: 120.0,
    qty_available: 5,
    categ_id: "Chambres",
    max_guests: 2
  },
  {
    id: 2,
    name: "Suite Deluxe",
    default_code: "SUITE-DELUXE",
    list_price: 250.0,
    qty_available: 2,
    categ_id: "Suites",
    max_guests: 4
  },
  {
    id: 3,
    name: "Chambre simple",
    default_code: "ROOM-S-01",
    list_price: 80.0,
    qty_available: 10,
    categ_id: "Chambres",
    max_guests: 1
  }
];

// =========================
// Affichage des produits dans la grille
// =========================
function renderProducts(products) {
  const container = document.getElementById('productsContainer');
  container.innerHTML = ""; // on vide la grille avant de la remplir

  if (!products || products.length === 0) {
    container.textContent = "Aucun produit à afficher.";
    return;
  }

  for (const p of products) {
    const card = document.createElement('article');
    card.className = 'product-card';

    card.innerHTML = `
      <h2>${p.name}</h2>
      <p><strong>ID :</strong> ${p.id}</p>
      <p><strong>Référence :</strong> ${p.default_code ?? "N/A"}</p>
      <p><strong>Prix :</strong> ${
        p.list_price?.toFixed ? p.list_price.toFixed(2) : p.list_price
      } €</p>
      <p><strong>Quantité disponible :</strong> ${p.qty_available ?? 0}</p>
      <p><strong>Catégorie :</strong> ${p.categ_id ?? "N/A"}</p>
      <p><strong>Max guests :</strong> ${p.max_guests ?? "N/A"}</p>
    `;

    container.appendChild(card);
  }
}

// =========================
// Gestion du clic sur "Charger les produits"
// =========================
document.getElementById('loadBtn').addEventListener('click', async () => {
  const status = document.getElementById('status');

  status.textContent = "Tentative d'authentification...";

  // 1) Authentification auprès de Odoo
  const ok = await authenticate();
  if (!ok) {
    // Si l'authentification échoue, on ne va pas plus loin
    return;
  }

  // 2) Ici, plus tard, tu feras un VRAI appel à Odoo pour récupérer les produits.
  // Pour l’instant on affiche des produits simulés.
  renderProducts(fakeProducts);
  status.textContent =
    "Produits chargés (simulation) : " + fakeProducts.length + " produits affichés.";
});
