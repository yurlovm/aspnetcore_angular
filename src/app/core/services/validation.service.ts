import { Injectable }           from '@angular/core';
import { AbstractControl }      from '@angular/forms';

@Injectable()
export class ValidationService {

    constructor() { }

    public emailValidator(control: AbstractControl): {[key: string]: boolean} {
        if ( this.emailMatch(control.value) ) {
            return null;
        } else {
            return { 'invalidEmailAddress': true };
        }
    }

    public getValidatorErrorMessage(validatorName: string, validatorValue?: any) {
        let config = {
            'required': 'Заполните это поле',
            'invalidEmailAddress': 'Неправильный формат Email',
            'invalidPassword': 'Invalid password. Password must be at least 8 characters long, and contain a number.',
            'minlength': `Минимальная длина ${validatorValue.requiredLength}`,
        };

        return config[validatorName];
    }

    private emailMatch(value: any) {
      // RFC 2822 compliant regex
      return value.match(/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/);
    }
}
