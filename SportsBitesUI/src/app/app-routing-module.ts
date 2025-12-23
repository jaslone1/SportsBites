import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EventListComponent } from './components/event-list/event-list.component';
import { EventDetailsComponent } from './components/event-details/event-details.component';
//import { CreateEventComponent } from './components/create-event/create-event.component';

const routes: Routes = [
  { path: 'events', component: EventListComponent },
  //{ path: 'event/new', component: CreateEventComponent },
  { path: 'event/:id', component: EventDetailsComponent },
  { path: '', redirectTo: '/events', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
