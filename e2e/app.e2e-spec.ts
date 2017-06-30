import { Ng2cliPage } from './app.po';

describe('ng2cli App', () => {
  let page: Ng2cliPage;

  beforeEach(() => {
    page = new Ng2cliPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
