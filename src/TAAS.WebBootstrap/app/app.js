$(function() {
    var menuItems = [{
        "name": "Accounts",
        "action": "accounts",
        "className": "pure-menu-selected"
    }];

    var MenuItem = React.createClass({
        render: function() {
            return(
                <li className={this.props.className}>
                    <a href="#">{this.props.name}</a>
                </li>);
        }
    });

    var MenuList = React.createClass({
        render: function() {
            var itemNodes = this.props.items.map(function(item) {
                return (
                    <MenuItem name={item.name} className={item.className}/>
                );
            });
            return (
                <nav id="nav-items" className="pure-menu pure-menu-blackbg pure-menu-open nav-inner">
                    <a href="#" className="pure-menu-heading">TAAS Admin</a>
                    <ul>{itemNodes}</ul>
                </nav>);
        }
    });

    var menu = $("#nav");
    React.render(<MenuList items={menuItems}/>, menu[0]);
});