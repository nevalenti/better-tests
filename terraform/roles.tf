resource "keycloak_role" "admin" {
  realm_id    = keycloak_realm.better_tests.id
  name        = "admin"
  description = "Administrator role"

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_role" "user" {
  realm_id    = keycloak_realm.better_tests.id
  name        = "user"
  description = "User role"

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_role" "viewer" {
  realm_id    = keycloak_realm.better_tests.id
  name        = "viewer"
  description = "Viewer role"

  depends_on = [keycloak_realm.better_tests]
}
