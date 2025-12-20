// Appelle l'API backend (port 5500 par défaut, adapter si différent).
const API_URL = "http://localhost:5500/api/products/loads";

document.getElementById("loadBtn").addEventListener("click", loadProducts);

async function loadProducts() {
    const status = document.getElementById("status");
    status.textContent = "Chargement en cours...";

    try {
        const payload = {
            url: document.getElementById("url").value,
            db: document.getElementById("db").value,
            login: document.getElementById("user").value,
            password: document.getElementById("password").value
        };

        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const text = await response.text();
        let data;
        try {
            data = text ? JSON.parse(text) : null;
        } catch (err) {
            throw new Error("Réponse non JSON : " + (text || "vide"));
        }

        if (!response.ok) {
            throw new Error((data && data.message) || text || "Erreur inconnue");
        }

        status.textContent = "Produits chargés : " + (Array.isArray(data) ? data.length : 0);
        renderProducts(Array.isArray(data) ? data : []);
    } catch (e) {
        if (e.message.includes("Failed to fetch") || e.message.includes("NetworkError")) {
            status.textContent = "Erreur de connexion : Impossible de joindre le serveur backend. Vérifiez que le serveur est démarré sur " + API_URL;
        } else {
            status.textContent = "Erreur : " + e.message;
        }
        console.error("Erreur détaillée:", e);
    }
}

function renderProducts(products) {
    const container = document.getElementById("productsContainer");
    container.innerHTML = "";

    if (!products || products.length === 0) {
        container.textContent = "Aucun produit à afficher.";
        return;
    }

    for (const p of products) {
        const card = document.createElement("article");
        card.className = "product-card";

        card.innerHTML = `
      <h2>${p.name ?? "-"}</h2>
      <p><strong>Référence :</strong> ${p.defaultCode ?? "-"}</p>
      <p><strong>Prix :</strong> ${p.listPrice ?? "-"}</p>
      <p><strong>Type :</strong> ${p.type ?? "-"}</p>
      <p><strong>Catégorie :</strong> ${formatCateg(p.categId)}</p>
      <p><strong>Quantité disponible :</strong> ${p.qtyAvailable ?? "-"}</p>
    `;

        container.appendChild(card);
    }
}

function formatCateg(categId) {
    if (Array.isArray(categId) && categId.length >= 2) return categId[1];
    return "-";
}
