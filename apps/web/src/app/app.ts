import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FooterComponent } from './layout/footer/footer.component';
import { HeaderComponent } from './layout/header/header.component';
import { CookiesComponent } from './shared/components/cookies/cookies.component';

@Component({
  imports: [RouterModule, HeaderComponent, FooterComponent, CookiesComponent],
  selector: 'app-root',
  templateUrl: 'app.html',
})
export class App {
  protected title = 'web';
}
