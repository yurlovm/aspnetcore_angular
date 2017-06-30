import { NgModule }                       from '@angular/core';
import { CommonModule }                   from '@angular/common';
import { FormsModule }                    from '@angular/forms';
import { NotFoundComponent }              from './not-found.component';
import { ControlErrorMessagesComponent }  from './control-error-messages.component';
import { ComposeMessageComponent }        from './compose-message/compose-message.component';
import { MdProgressSpinnerModule }        from '@angular/material';
import { MdButtonModule }                 from '@angular/material';

@NgModule({
  imports:      [ CommonModule, FormsModule, MdProgressSpinnerModule, MdButtonModule ],
  declarations: [ NotFoundComponent, ComposeMessageComponent, ControlErrorMessagesComponent ],
  exports:      [ CommonModule, FormsModule, NotFoundComponent, ComposeMessageComponent, ControlErrorMessagesComponent,
                  MdProgressSpinnerModule, MdButtonModule ],
})
export class SharedModule { }
