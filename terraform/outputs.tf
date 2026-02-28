output "realm_id" {
  description = "Keycloak realm ID"
  value       = keycloak_realm.better_tests.id
}

output "api_client_id" {
  description = "API client ID"
  value       = keycloak_openid_client.api.client_id
}

output "web_client_id" {
  description = "Web client ID"
  value       = keycloak_openid_client.web.client_id
}

output "web_client_secret" {
  description = "Web client secret"
  value       = keycloak_openid_client.web.client_secret
  sensitive   = true
}

output "keycloak_realm_url" {
  description = "Keycloak realm URL"
  value       = "${var.keycloak_url}/realms/${keycloak_realm.better_tests.realm}"
}

output "test_users" {
  description = "Created test users"
  value = {
    for username, user in keycloak_user.test_users : username => {
      email = user.email
      id    = user.id
    }
  }
}

output "roles" {
  description = "Created roles"
  value = {
    admin  = keycloak_role.admin.id
    user   = keycloak_role.user.id
    viewer = keycloak_role.viewer.id
  }
}
