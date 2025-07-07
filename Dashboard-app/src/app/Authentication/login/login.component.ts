import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../common/auth/auth.service';
import { Router } from '@angular/router';
import { ILogin, ILoginByUserName } from '../../common/Models/IAuth';
import { FormsModule, NgForm } from '@angular/forms';
import { NgModule } from '@angular/core';
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
  selector: 'app-login',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    FormsModule,
    CommonModule,
    NgbAlertModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  loginData!: ILogin;
  loginByName!: ILoginByUserName;
  loginByEmail: boolean = true;
  alert: Alert | null = null;

  ngOnInit(): void {}

  constructor(private router: Router, private authService: AuthService) {}

  close() {
    this.alert = null; // Hides the alert when closed
  }

  change() {
    this.loginByEmail = !this.loginByEmail;
  }
  loginByUser(form: NgForm) {
    this.loginByName = { ...form.value };
    console.log(this.loginByName);
    this.authService.loginByUser(this.loginByName).subscribe({
      next: (res) => {
        this.router.navigate(['home']);
      },
      error: (err) => {
        console.error('Login failed', err);
        this.alert = ALERT;
        this.alert.message = err.error;
      },
    });
  }
  login(form: NgForm) {
    this.loginData = { ...form.value };
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        this.router.navigate(['home']);
      },
      error: (err) => {
        console.error('Login failed', err);
        this.alert = ALERT;
        this.alert.message = err.error;
      },
    });
  }
}
