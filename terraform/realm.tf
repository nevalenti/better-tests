resource "keycloak_realm" "better_tests" {
  realm   = var.realm_name
  enabled = true

  account_theme = "base"
  admin_theme   = "base"
  login_theme   = "better-tests"
  email_theme   = "base"

  internationalization {
    supported_locales = [
      "en",
    ]
    default_locale = "en"
  }

  password_policy = "length(8) and specialChars(1) and upperCase(1) and digits(1)"

  smtp_server {
    host = "mailhog"
    port = 1025
    from = "noreply@better-tests.local"
  }

  registration_allowed           = true
  registration_email_as_username = true
  reset_password_allowed         = true
  remember_me                    = false
  verify_email                   = false
  login_with_email_allowed       = true
  duplicate_emails_allowed       = false
}
