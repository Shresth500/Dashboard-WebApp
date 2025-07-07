import { Component, Inject, inject } from '@angular/core';
import {
  MatSnackBar,
  MatSnackBarAction,
  MatSnackBarActions,
  MatSnackBarLabel,
  MatSnackBarRef,
} from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-snackbar',
  imports: [
    MatButtonModule,
    MatSnackBarLabel,
    MatSnackBarActions,
    MatSnackBarAction,
    CommonModule,
  ],
  templateUrl: './snackbar.component.html',
  styleUrl: './snackbar.component.scss',
})
export class SnackbarComponent {
  snackBarRef = inject(MatSnackBarRef);
  constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) {}
}
