variable "keycloak_url" {
  description = "Keycloak URL"
  type        = string
  default     = "http://localhost:8080"
}

variable "keycloak_admin_username" {
  description = "Keycloak admin username"
  type        = string
  default     = "admin"
}

variable "keycloak_admin_password" {
  description = "Keycloak admin password"
  type        = string
  sensitive   = true
  default     = "admin"
}

variable "keycloak_admin_client_id" {
  description = "Keycloak admin client ID"
  type        = string
  default     = "admin-cli"
}

variable "realm_name" {
  description = "Keycloak realm name"
  type        = string
  default     = "better-tests"
}

variable "api_client_id" {
  description = "API client ID"
  type        = string
  default     = "better-reads-api"
}

variable "web_client_id" {
  description = "Web application client ID"
  type        = string
  default     = "better-reads-web"
}

variable "web_redirect_uris" {
  description = "Web app redirect URIs"
  type        = list(string)
  default     = ["http://localhost:4200/*"]
}

variable "web_origins" {
  description = "Web app CORS origins"
  type        = list(string)
  default     = ["http://localhost:4200"]
}

variable "api_redirect_uris" {
  description = "API redirect URIs"
  type        = list(string)
  default     = ["http://localhost:5048/*"]
}

variable "test_users" {
  description = "Test users to create"
  type = map(object({
    email      = string
    first_name = string
    last_name  = string
    password   = string
  }))
}

variable "google_client_id" {
  description = "Google OAuth 2.0 Client ID"
  type        = string
  default     = ""
  sensitive   = false
}

variable "google_client_secret" {
  description = "Google OAuth 2.0 Client Secret"
  type        = string
  default     = ""
  sensitive   = true
}

variable "github_client_id" {
  description = "GitHub OAuth App Client ID"
  type        = string
  default     = ""
  sensitive   = false
}

variable "github_client_secret" {
  description = "GitHub OAuth App Client Secret"
  type        = string
  default     = ""
  sensitive   = true
}
