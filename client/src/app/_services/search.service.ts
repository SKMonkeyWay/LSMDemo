import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  baseUrl = 'https://localhost:44369/api/search/'; 

  constructor(private http: HttpClient) {  }
  search(model: any){
    return this.http.post(this.baseUrl, model);
  }
}
