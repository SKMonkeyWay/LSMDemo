import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'LSMDemo';
  data: any

  constructor(private http: HttpClient){

  }
  ngOnInit() {
    this.getData()
  }
  getData(){
    this.http.get('https://localhost:5001/api/search').subscribe(response => {
      this.data = response;
    },error => {
      console.log(error);
    })
  }
}