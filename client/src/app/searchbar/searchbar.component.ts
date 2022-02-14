import { Component, OnInit } from '@angular/core';
import { SearchService } from '../_services/search.service';

@Component({
  selector: 'app-searchbar',
  templateUrl: './searchbar.component.html',
  styleUrls: ['./searchbar.component.css']
})
export class SearchbarComponent implements OnInit {
  model: any = {}
  isSearchList: boolean = false;
  public buildings:any[];
  public locks: any[];
  public groups: any[];
  public media: any[];

  isBuilding: boolean = false;
  isLocks: boolean = false;
  isGroups: boolean = false;
  isMedia: boolean = false;

  constructor(public searchService: SearchService) { }

  ngOnInit(): void {

  }

  onSubmit(){
    this.searchService.search(this.model).subscribe(response =>{
      let result = <any>response
      if(result !=null){
        this.isSearchList = true;
      }      
        this.buildings = result.buildings;
        this.locks = result.locks;
        this.groups = result.groups;
        this.media = result.media;
    })
  }
}
