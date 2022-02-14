import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { buildings } from '../_models/buildings';
import { groups } from '../_models/groups';
import { locks } from '../_models/locks';
import { media } from '../_models/media';
import { root } from '../_models/root';

@Component({
  selector: 'app-serchbar',
  templateUrl: './serchbar.component.html',
  styleUrls: ['./serchbar.component.css']
})
export class SerchbarComponent implements OnInit {
  // data: root = {
  //   buildings: [],
  //   locks: [],
  //   groups: [],
  //   media: []
  // }
  buildings:any [];
  locks: locks[];
  groups: groups[];
  media: media[];
  data: any[];
  searchquery: string ='';
  searchUrl: string = ''

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }


  onSubmit(searquery: string){
    this.searchUrl = 'https://localhost:44369/api/search?Searchquery='+[searquery];
    this.http.get<root[]>(this.searchUrl).subscribe(response => {
      debugger;
      this.data = response;
      this.buildings =(<any>response).buildings;
      console.log(this.data);
      // this.data.forEach(item => {
      //   this.buildings = item.buildings;
      // });

      console.log(this.data)
    },error => {
      console.log(error);    
    })
  }
}
