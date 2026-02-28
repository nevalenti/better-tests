import { createApplication } from '@angular/platform-browser';

import { App } from './app/app';
import { appConfig } from './app/app.config';

createApplication(appConfig)
  .then((app) => {
    app.bootstrap(App);
  })
  .catch((err) => console.error(err));
