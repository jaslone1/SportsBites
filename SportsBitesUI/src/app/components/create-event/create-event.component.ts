import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  standalone: false
})
export class CreateEventComponent {
  // This object matches what your .NET API expects
  newEvent = {
    eventName: '',
    eventDate: '',
    gameDetails: '',
    hostName: 'Jared' // Defaulting to you!
  };

  constructor(private eventService: EventService, private router: Router) {}

  saveEvent() {
    this.eventService.createEvent(this.newEvent).subscribe({
      next: () => {
        alert('Touchdown! Event created.');
        this.router.navigate(['/events']); // Send user back to the list
      },
      error: (err) => console.error('Error creating event:', err)
    });
  }
}
