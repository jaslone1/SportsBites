import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDto } from '../../models/event';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  standalone: false
})
export class HomePageComponent implements OnInit {
  upcomingEvents: any[] = [];

  constructor(
    private eventService: EventService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getEvents().subscribe({
      next: (data: any[]) => {
        const now = new Date();

        this.upcomingEvents = data.filter(e => new Date(e.eventDate) >= now);

        console.log('Events Split:', this.upcomingEvents.length );

        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('List load failed', err);
      }
    });
  }
}
