import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchbarComponent } from './searchbar/searchbar.component';
import { SearchlistComponent } from './searchlist/searchlist.component';

const routes: Routes = [
  {path: '', component: SearchbarComponent },
  {path: 'searchlist', component: SearchlistComponent },
  {path: '**', component:SearchbarComponent, pathMatch :'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
