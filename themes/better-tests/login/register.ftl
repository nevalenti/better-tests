<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=displayMessage>
  <div class="card from-base-100 to-base-200/30 border-base-content/10 w-full rounded-lg border bg-linear-to-b shadow-lg">
    <div class="card-body px-6 py-8 sm:px-8 sm:py-10">
      <div class="mb-6">
        <h1 class="text-center">
          <span class="text-base-content text-3xl font-extrabold whitespace-nowrap" style="font-family: var(--font-display)">Create an Account</span>
        </h1>
        <p class="text-center text-sm text-base-content/60 mt-2">Join us to get started</p>
      </div>

      <#if message??>
        <div class="alert alert-error alert-outline border-dashed mb-6 gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-5 w-5" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          <span class="text-sm whitespace-pre-wrap">${message.summary?replace('<br/>', '\n')?replace('<br>', '\n')}</span>
        </div>
      </#if>

      <form id="kc-register-form" onsubmit="register.disabled = true; return true;" action="${url.registrationAction}" method="post" class="space-y-4">
        <div class="grid grid-cols-2 gap-3">
          <div class="form-group">
            <label for="firstName" class="block text-sm font-semibold mb-2.5 text-base-content">First Name</label>
            <div class="relative">
              <input
                id="firstName"
                name="firstName"
                value="${(register.formData.firstName!'')}"
                type="text"
                placeholder="John"
                class="input input-bordered w-full h-12 text-sm focus:input-primary transition-colors"
              />
            </div>
          </div>
          <div class="form-group">
            <label for="lastName" class="block text-sm font-semibold mb-2.5 text-base-content">Last Name</label>
            <div class="relative">
              <input
                id="lastName"
                name="lastName"
                value="${(register.formData.lastName!'')}"
                type="text"
                placeholder="Doe"
                class="input input-bordered w-full h-12 text-sm focus:input-primary transition-colors"
              />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label for="email" class="block text-sm font-semibold mb-2.5 text-base-content">Email Address</label>
          <div class="relative">
            <input
              id="email"
              name="email"
              value="${(register.formData.email!'')}"
              type="email"
              autocomplete="email"
              placeholder="john@example.com"
              class="input input-bordered w-full h-12 text-sm focus:input-primary transition-colors"
            />
            <svg class="absolute right-3.5 top-1/2 -translate-y-1/2 w-5 h-5 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>
          </div>
        </div>

        <div class="form-group">
          <label for="password" class="block text-sm font-semibold mb-2.5 text-base-content">Password</label>
          <div class="relative">
            <input
              id="password"
              name="password"
              type="password"
              autocomplete="new-password"
              placeholder="••••••••"
              class="input input-bordered w-full h-12 text-sm focus:input-primary transition-colors pr-12"
            />
            <svg class="absolute right-3.5 top-1/2 -translate-y-1/2 w-5 h-5 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2" /><path d="M7 11V7a5 5 0 0110 0v4" /></svg>
          </div>
        </div>

        <div class="form-group">
          <label for="password-confirm" class="block text-sm font-semibold mb-2.5 text-base-content">Confirm Password</label>
          <div class="relative">
            <input
              id="password-confirm"
              name="password-confirm"
              type="password"
              placeholder="••••••••"
              class="input input-bordered w-full h-12 text-sm focus:input-primary transition-colors pr-12"
            />
            <svg class="absolute right-3.5 top-1/2 -translate-y-1/2 w-5 h-5 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          </div>
        </div>

        <button
          id="kc-register"
          name="register"
          type="submit"
          value="Register"
          class="btn btn-primary w-full h-12 font-semibold text-base mt-6 hover:shadow-lg transition-all duration-200"
        >
          Create an account
        </button>
      </form>

      <div class="divider my-6"></div>

      <div class="space-y-2.5">
        <#if social?? && social.providers??>
          <#list social.providers as provider>
            <#if provider.alias == 'google'>
              <a
                href="${provider.loginUrl}"
                class="btn btn-soft h-12 w-full justify-center gap-2.5 text-sm font-medium transition-all duration-200 hover:shadow-md"
              >
                <svg class="size-5" viewBox="0 0 24 24">
                  <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" />
                  <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" />
                  <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" />
                  <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" />
                </svg>
                Continue with Google
              </a>
            </#if>
            <#if provider.alias == 'github'>
              <a
                href="${provider.loginUrl}"
                class="btn btn-soft h-12 w-full justify-center gap-2.5 text-sm font-medium transition-all duration-200 hover:shadow-md"
              >
                <svg class="size-5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
                </svg>
                Continue with GitHub
              </a>
            </#if>
          </#list>
        </#if>
      </div>

      <p class="text-center text-sm text-base-content/70 mt-7">
        Already have an account?
        <a href="${url.loginUrl}" class="link link-accent font-semibold">
          Log in
        </a>
      </p>
    </div>
  </div>
</@layout.registrationLayout>
