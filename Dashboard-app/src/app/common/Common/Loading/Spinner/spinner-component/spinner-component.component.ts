import { Component } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
@Component({
  selector: 'app-spinner-component',
  imports: [MatProgressSpinnerModule],
  templateUrl: './spinner-component.component.html',
  styleUrl: './spinner-component.component.scss',
})
export class SpinnerComponentComponent {}
