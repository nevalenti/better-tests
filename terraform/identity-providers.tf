resource "keycloak_oidc_google_identity_provider" "google" {
  realm          = keycloak_realm.better_tests.realm
  client_id      = var.google_client_id
  client_secret  = var.google_client_secret
  enabled        = var.google_client_id != "" ? true : false
  trust_email    = true
  store_token    = true
  default_scopes = "openid profile email"

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_oidc_identity_provider" "github" {
  realm        = keycloak_realm.better_tests.realm
  provider_id  = "github"
  alias        = "github"
  display_name = "GitHub"
  enabled      = var.github_client_id != "" ? true : false

  authorization_url = "https://github.com/login/oauth/authorize"
  token_url         = "https://github.com/login/oauth/access_token"
  user_info_url     = "https://api.github.com/user"

  client_id     = var.github_client_id
  client_secret = var.github_client_secret

  default_scopes = "user:email"
  trust_email    = true
  store_token    = true

  depends_on = [keycloak_realm.better_tests]
}
