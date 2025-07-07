import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../common/auth/auth.service';
import { Router } from '@angular/router';
import { IRegister } from '../../common/Models/IAuth';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';

interface Alert {
  type: string;
  message: string;
}

const ALERT: Alert = {
  type: 'danger',
  message: 'This is a danger alert',
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    CommonModule,
    FormsModule,
    NgbAlertModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  user!: IRegister;
  alertMessage: string | null = null;
  alert: Alert | null = null;

  close() {
    this.alert = null; // Hides the alert when closed
  }

  ngOnInit(): void {}
  constructor(private router: Router, private authService: AuthService) {}
  register(form: NgForm) {
    let data = { ...form.value };
    let temp: IRegister = {
      username: data.email,
      email: data.email,
      password: data.email,
    };
    if (data.password !== data.confirmPassword) {
      this.alert = ALERT;
      this.alert.message = 'Password and Confirm Password are not matching';
      return;
    }
    this.user = temp;
    console.log(this.user);
    this.authService.register(this.user).subscribe({
      next: (resp) => {
        console.log(resp);
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        console.error('Registration error:', err);
        this.alert = ALERT;
        this.alert.message = err.error;
      },
    });
  }
}
