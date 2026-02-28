<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=displayMessage displayRequiredFields=false>
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

      <form id="kc-form-login" onsubmit="login.disabled = true; return true;" action="${url.loginAction}" method="post" class="space-y-6">
        <div class="form-group">
          <label for="username" class="block text-xs font-semibold mb-2 text-base-content">Email Address</label>
          <div class="relative">
            <input
              id="username"
              name="username"
              value="${(login.username!'')}"
              type="text"
              autofocus
              autocomplete="username"
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
              autocomplete="current-password"
              placeholder="••••••••"
              class="input input-bordered w-full text-sm focus:input-primary transition-colors pr-10"
            />
            <svg class="absolute right-4 top-1/2 -translate-y-1/2 w-4 h-4 opacity-40 pointer-events-none" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="11" width="18" height="11" rx="2" ry="2" /><path d="M7 11V7a5 5 0 0110 0v4" /></svg>
          </div>
        </div>

        <#if realm.rememberMe>
          <div class="flex items-center gap-3 py-2">
            <input
              id="rememberMe"
              name="rememberMe"
              type="checkbox"
              value="on"
              class="checkbox checkbox-sm"
            />
            <label for="rememberMe" class="label-text text-xs cursor-pointer">Keep me logged in</label>
          </div>
        </#if>

        <button
          id="kc-login"
          name="login"
          type="submit"
          value="Sign In"
          class="btn btn-primary w-full font-semibold text-sm mt-8 hover:shadow-lg transition-all duration-200"
        >
          Log in
        </button>
      </form>

      <div class="divider my-6">OR</div>

      <#if realm.password && realm.registrationAllowed && !usernameEditDisabled??>
        <a href="${url.registrationUrl}" class="link link-accent font-semibold text-center block">
          Don't have an account?
        </a>
      </#if>
    </div>
  </div>
</@layout.registrationLayout>
