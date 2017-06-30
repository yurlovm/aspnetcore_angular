import { Component, Input }       from '@angular/core';
import { FormControl }            from '@angular/forms';
import { ValidationService }      from '../core/services/validation.service';

@Component({
  selector: 'my-control-error-messages',
  template: `<div *ngIf="errorMessage !== null">{{errorMessage}}</div>`,
})
export class ControlErrorMessagesComponent {
  @Input() public control: FormControl;
  constructor(private validationService: ValidationService) { }

  get errorMessage() {
    for (let propertyName in this.control.errors) {
      if (this.control.errors.hasOwnProperty(propertyName) && this.control.touched) {
        return this.validationService.getValidatorErrorMessage(propertyName, this.control.errors[propertyName]);
      }
    }
    return null;
  }
}
