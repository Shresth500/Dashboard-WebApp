import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  NgForm,
  FormsModule,
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { IAddproduct } from '../../../common/Models/IAuth';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './add-product.component.html',
  styleUrl: './add-product.component.scss',
})
export class AddProductComponent implements OnInit {
  @Output() addProduct = new EventEmitter<IAddproduct>();

  ngOnInit(): void {}
  userForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      status: ['Pending', Validators.required], // Default value is "online"
    });
  }
  onSubmit() {
    if (this.userForm.valid) {
      console.log('Form Data:', { ...this.userForm.value });
      let data: IAddproduct = { ...this.userForm.value };
      this.addProduct.emit(data);
      this.userForm.reset();
    }
  }
}
