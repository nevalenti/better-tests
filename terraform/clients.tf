resource "keycloak_openid_client" "api" {
  realm_id              = keycloak_realm.better_tests.id
  client_id             = var.api_client_id
  name                  = "API Client"
  enabled               = true
  access_type           = "BEARER-ONLY"
  standard_flow_enabled = false

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_openid_client" "web" {
  realm_id                     = keycloak_realm.better_tests.id
  client_id                    = var.web_client_id
  name                         = "Web Application"
  enabled                      = true
  access_type                  = "PUBLIC"
  standard_flow_enabled        = true
  implicit_flow_enabled        = false
  direct_access_grants_enabled = true
  pkce_code_challenge_method   = "S256"

  valid_redirect_uris = var.web_redirect_uris
  web_origins         = var.web_origins

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_openid_client_default_scopes" "web_default_scopes" {
  realm_id  = keycloak_realm.better_tests.id
  client_id = keycloak_openid_client.web.id
  default_scopes = [
    "profile",
    "email"
  ]

  depends_on = [keycloak_openid_client.web]
}

resource "keycloak_generic_role_mapper" "web_roles" {
  realm_id  = keycloak_realm.better_tests.id
  client_id = keycloak_openid_client.web.id

  role_id = keycloak_role.user.id

  depends_on = [keycloak_openid_client.web, keycloak_role.user]
}
