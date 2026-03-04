resource "keycloak_openid_client" "better-tests-api" {
  realm_id              = keycloak_realm.better_tests.id
  client_id             = var.api_client_id
  name                  = "API Client"
  enabled               = true
  access_type           = "BEARER-ONLY"
  standard_flow_enabled = false

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_openid_client" "better-tests-web" {
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
  client_id = keycloak_openid_client.better-tests-web.id
  default_scopes = [
    "openid",
    "profile",
    "email"
  ]

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_protocol_mapper" "api_audience_mapper" {
  realm_id        = keycloak_realm.better_tests.id
  client_id       = keycloak_openid_client.better-tests-web.id
  name            = "api-audience-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-audience-mapper"
  config = {
    "included.client.audience"  = var.api_client_id
    "access.token.claim"        = "true"
    "introspection.token.claim" = "true"
  }

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_protocol_mapper" "subject_claim_mapper" {
  realm_id        = keycloak_realm.better_tests.id
  client_id       = keycloak_openid_client.better-tests-web.id
  name            = "subject-claim-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-usermodel-property-mapper"
  config = {
    "user.attribute"           = "username"
    "claim.name"               = "sub"
    "access.token.claim"       = "true"
    "id.token.claim"           = "true"
    "userinfo.token.claim"     = "true"
    "introspection.token.claim" = "true"
  }

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_protocol_mapper" "email_mapper" {
  realm_id        = keycloak_realm.better_tests.id
  client_id       = keycloak_openid_client.better-tests-web.id
  name            = "email-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-usermodel-property-mapper"
  config = {
    "usermodel.property.name"  = "email"
    "claim.name"               = "email"
    "access.token.claim"       = "true"
    "id.token.claim"           = "true"
    "userinfo.token.claim"     = "true"
    "introspection.token.claim" = "true"
  }

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_protocol_mapper" "given_name_mapper" {
  realm_id        = keycloak_realm.better_tests.id
  client_id       = keycloak_openid_client.better-tests-web.id
  name            = "given-name-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-usermodel-property-mapper"
  config = {
    "usermodel.property.name"  = "firstName"
    "claim.name"               = "given_name"
    "access.token.claim"       = "true"
    "id.token.claim"           = "true"
    "userinfo.token.claim"     = "true"
    "introspection.token.claim" = "true"
  }

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_protocol_mapper" "family_name_mapper" {
  realm_id        = keycloak_realm.better_tests.id
  client_id       = keycloak_openid_client.better-tests-web.id
  name            = "family-name-mapper"
  protocol        = "openid-connect"
  protocol_mapper = "oidc-usermodel-property-mapper"
  config = {
    "usermodel.property.name"  = "lastName"
    "claim.name"               = "family_name"
    "access.token.claim"       = "true"
    "id.token.claim"           = "true"
    "userinfo.token.claim"     = "true"
    "introspection.token.claim" = "true"
  }

  depends_on = [keycloak_openid_client.better-tests-web]
}

resource "keycloak_generic_role_mapper" "web_roles" {
  realm_id  = keycloak_realm.better_tests.id
  client_id = keycloak_openid_client.better-tests-web.id

  role_id = keycloak_role.user.id

  depends_on = [keycloak_openid_client.better-tests-web, keycloak_role.user]
}
