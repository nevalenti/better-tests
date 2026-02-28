<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=displayMessage>
  <div class="card from-base-100 to-base-200/30 border-base-content/10 w-full max-w-md rounded-lg border bg-linear-to-b shadow-lg">
    <div class="card-body px-8 py-12">
      <div class="mb-8">
        <h1 class="text-center">
          <span class="text-base-content text-4xl font-extrabold whitespace-nowrap" style="font-family: var(--font-display)">Better Tests</span>
        </h1>
      </div>

      <#if message??>
        <div class="alert alert-error alert-outline border-dashed mb-6 gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-5 w-5" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          <span class="text-sm whitespace-pre-wrap">${message.summary?replace('<br/>', '\n')?replace('<br>', '\n')}</span>
        </div>
      </#if>

      <form id="kc-register-form" onsubmit="register.disabled = true; return true;" action="${url.registrationAction}" method="post" class="space-y-6">
        <div class="grid grid-cols-2 gap-5">
          <div class="form-group">
            <label for="firstName" class="block text-xs font-semibold mb-2 text-base-content">First Name</label>
            <div class="relative">
              <input
                id="firstName"
                name="firstName"
                value="${(register.formData.firstName!'')}"
                type="text"
                placeholder="John"
                class="input input-bordered w-full text-sm focus:input-primary transition-colors"
              />
            </div>
          </div>
          <div class="form-group">
            <label for="lastName" class="block text-xs font-semibold mb-2 text-base-content">Last Name</label>
            <div class="relative">
              <input
                id="lastName"
                name="lastName"
                value="${(register.formData.lastName!'')}"
                type="text"
                placeholder="Doe"
                class="input input-bordered w-full text-sm focus:input-primary transition-colors"
              />
            </div>
          </div>
        </div>

        <div class="form-group">
          <label for="email" class="block text-xs font-semibold mb-2 text-base-content">Email Address</label>
          <div class="relative">
            <input
              id="email"
              name="email"
              value="${(register.formData.email!'')}"
              type="email"
              autocomplete="email"
              placeholder="john@example.com"
              class="input input-bordered w-full text-sm focus:input-primary transition-colors"
            />
            <svg class="absolute right-4 top-1/2 -translate-y-1/2 w-4 h-4 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>
          </div>
        </div>

        <div class="form-group">
          <label for="password" class="block text-xs font-semibold mb-2 text-base-content">Password</label>
          <div class="relative">
            <input
              id="password"
              name="password"
              type="password"
              autocomplete="new-password"
              placeholder="••••••••"
              class="input input-bordered w-full text-sm focus:input-primary transition-colors pr-10"
            />
            <svg class="absolute right-4 top-1/2 -translate-y-1/2 w-4 h-4 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2" /><path d="M7 11V7a5 5 0 0110 0v4" /></svg>
          </div>
        </div>

        <div class="form-group">
          <label for="password-confirm" class="block text-xs font-semibold mb-2 text-base-content">Confirm Password</label>
          <div class="relative">
            <input
              id="password-confirm"
              name="password-confirm"
              type="password"
              placeholder="••••••••"
              class="input input-bordered w-full text-sm focus:input-primary transition-colors pr-10"
            />
            <svg class="absolute right-4 top-1/2 -translate-y-1/2 w-4 h-4 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          </div>
        </div>

        <button
          id="kc-register"
          name="register"
          type="submit"
          value="Register"
          class="btn btn-primary w-full font-semibold text-sm mt-8 hover:shadow-lg transition-all duration-200"
        >
          Create Account
        </button>
      </form>

      <div class="divider my-6">OR</div>

      <a href="${url.loginUrl}" class="link link-accent font-semibold text-center block">
        Already have an account?
      </a>
    </div>
  </div>
</@layout.registrationLayout>
