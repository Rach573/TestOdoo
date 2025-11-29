from odoo import models, fields


class RentalProperty(models.Model):
    _inherit = "product.template"

    # Informations de base
    max_guest_count = fields.Integer("Nombre maximum d'invités", default=0)
    bed_count = fields.Integer("Nombre de lits", default=0)
    bedroom_count = fields.Integer("Nombre de chambres", default=0)
    bathroom_count = fields.Integer("Nombre de salles de bain", default=0)

    # Informations d'adresse
    street = fields.Char("Rue")
    street_number = fields.Char("Numéro")
    zip_code = fields.Char("Code postal")

    # Équipements disponibles
    has_air_conditioning = fields.Boolean("Climatisation")
    has_terrace = fields.Boolean("Terrasse")
    has_garden = fields.Boolean("Jardin")
    has_swimming_pool = fields.Boolean("Piscine")
    has_jacuzzi = fields.Boolean("Jacuzzi")
    has_ev_charger = fields.Boolean("Chargeur EV")
    has_indoor_fireplace = fields.Boolean("Cheminée intérieure")
    has_outdoor_fireplace = fields.Boolean("Cheminée extérieure")
    has_dedicated_workspace = fields.Boolean("Espace de travail dédié")
    has_gym = fields.Boolean("Salle de sport")

    # Caractéristiques d'accessibilité
    has_toilet_grab_bar = fields.Boolean("Barre d'appui pour toilettes")
    has_shower_grab_bar = fields.Boolean("Barre d'appui pour douche")
    has_step_free_shower = fields.Boolean("Douche sans marche")
    has_shower_chair = fields.Boolean("Chaise de douche/bain")
    has_step_free_bedroom_access = fields.Boolean("Accès sans marche à la chambre")
    has_wide_bedroom_entry = fields.Boolean("Entrée large de chambre")
    has_general_step_free_access = fields.Boolean("Accès sans marche général")
