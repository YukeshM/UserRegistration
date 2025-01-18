import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  private config: any;

  constructor(private http: HttpClient) {}

  loadConfig(): Observable<any> {
    return this.http.get('assets/app-config.json');
  }

  // Assuming loadConfig() is called first before accessing the apiUrl.
  get apiUrl(): string {
    return this.config?.apiUrl || '';
  }
}
