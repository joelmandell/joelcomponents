# Progress Telerik Blazor Assesment
## By Joel Mandell


### Short description

1. Started off by implementing the "root-container"; ``<Menu>``-component. In the test project (JoelComponents.Web), there is a test page with the component failing without having a ```<Menu>```as root component.
2. ```MenuItems```-component for the support of submenu.
   * The first MenuItems, will be a horizontal container for the top-level root.
        * MenuItem in the top-level root will on hover (if it in its self has a MenuItems) expand a vertical "dropdown"
        * If there is nested MenuItems with one or more MenuItem components it will be expanded to right.

This solution is kinda like a hybrid variant, with both CSS and a JS-function for blur (when for example pressing Esc, on the keyboard).
I tried to just use Blazor. I think it would be possible by Cascading and keeping track on children and so on, that is by only using Blazor and for example with model binding and only rendering on hover on submenuitems and so on.

But it seemed quite fragile with the "tabbing"-experience and **ARIA**-support. So I went for CSS combined with one JS-function. But most of the events are done within the razor components.
Tabbing should work mostly okay.

I added two parameters for the Menu-component for OnClick or OnKeyEnter-events.

### Things I wanted to implement

1. Some bUnit tests would have been nice.
2. Template-tag for customizing the rendered text in a menu-item
3. A MenuIcon-component that could be prepend or appended.
4. An property/setting for changing it from horizontal to vertical (responsive ui)
5. A MenuTheme-container or something like that to theme the colors and fonts with css variables.
