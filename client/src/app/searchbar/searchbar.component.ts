import { Component, OnInit } from '@angular/core';
import { throwError } from 'rxjs';
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

  msgParagraph: boolean = false;  
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
      if(result.buildings.length > 0){
        this.buildings = result.buildings;
        this.isBuilding = true;
      }
      if(result.locks.length > 0){
        this.locks = result.locks;
        this.isLocks = true;
      }
      if(result.groups.length > 0){
        this.groups = result.groups;
      }
      if(result.media.length > 0){
        this.media = result.media;
        this.isMedia = true;
      }        
    }, error => {
     this.msgParagraph = true;
    });
  }
}
