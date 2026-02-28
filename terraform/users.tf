resource "keycloak_user" "test_users" {
  for_each = var.test_users

  realm_id = keycloak_realm.better_tests.id
  username = each.key
  email    = each.value.email
  enabled  = true

  first_name = each.value.first_name
  last_name  = each.value.last_name

  email_verified = true

  initial_password {
    value     = each.value.password
    temporary = false
  }

  depends_on = [keycloak_realm.better_tests]
}

resource "keycloak_user_roles" "admin_user_roles" {
  realm_id = keycloak_realm.better_tests.id
  user_id  = keycloak_user.test_users["administrator"].id

  role_ids = [
    keycloak_role.admin.id,
    keycloak_role.user.id,
  ]

  depends_on = [keycloak_user.test_users, keycloak_role.admin, keycloak_role.user]
}

