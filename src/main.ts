import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { AppConfigService } from './app/service/app-config.service';

// async function loadApp(){
//   const appConfigService = new AppConfigService();
//   await appConfigService.loadConfig();
bootstrapApplication(AppComponent, appConfig).catch((err) =>
  console.error(err)
);
// }

// loadApp();
