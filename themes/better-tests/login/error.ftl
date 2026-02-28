<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=displayMessage>
  <div class="card from-base-100 to-base-200/30 border-base-content/10 w-full max-w-sm rounded-lg border bg-linear-to-b shadow-lg">
    <div class="card-body px-8 py-10">
      <div class="mb-10 text-center">
        <div class="flex justify-center mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="w-16 h-16 text-error opacity-80" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10" /><path d="M12 8v4M12 16h.01" /></svg>
        </div>
        <h1 class="text-5xl font-extrabold whitespace-nowrap" style="font-family: var(--font-display)">Better Tests</h1>
        <p class="mt-4 text-center text-sm opacity-70">An unexpected error occurred</p>
      </div>

      <#if message??>
        <div class="alert alert-error alert-outline border-dashed mb-6 gap-3">
          <svg xmlns="http://www.w3.org/2000/svg" class="stroke-current shrink-0 h-5 w-5" fill="none" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
          <span class="text-sm">${message.summary}</span>
        </div>
      </#if>

      <p class="text-center text-sm opacity-70 mb-8">
        Please try again or contact our support team if the problem persists.
      </p>

      <a href="${url.loginUrl}" class="btn btn-primary w-full font-semibold text-sm mt-6">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12h14M12 5l7 7-7 7" /></svg>
        Return to Login
      </a>
    </div>
  </div>
</@layout.registrationLayout>
