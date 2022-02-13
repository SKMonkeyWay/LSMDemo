import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-serchbar',
  templateUrl: './serchbar.component.html',
  styleUrls: ['./serchbar.component.css']
})
export class SerchbarComponent implements OnInit {
  data: any

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  getData(){
    this.http.get('https://localhost:5001/api/search').subscribe(response => {
      this.data = response;
    },error => {
      console.log(error);
    })
  }

}
