// the followinf code was develop using the protoping tools : http://www.apcjones.com/arrows/#
// It was used as first attempt for prototype the database

CREATE 
  (`0` :Customer {Id:'001,',name:'Adam Smith',DoB:'1990-02-02',Gender:'Male',Mobile:'0785553221',Email:'some@some.com'}) ,
  (`1` :Order {Status:'Delivered',Date:'2017-01-15'}) ,
  (`2` :`Product B` {id:'1',Name:'PB',Brand:'Brand1',Color:'Red'}) ,
  (`3` :`Product A` {id:'2',Name:'PA',Brand:'Brand2',Color:'Black'}) ,
  (`4` :`Product C` {id:'6',Name:'PC',Brand:'Brand2',Color:'White'}) ,
  (`5` :`Product D` {id:'5',Name:'PD',Brand:'Brand2',Color:'Black'}) ,
  (`6` :`Product E` {id:'4',Name:'PA',Brand:'Brand2',Color:'Black'}) ,
  (`7` :`Product X` {id:'3',Name:'PX',Brand:'Brand2',Color:'Black'}) ,
  (`8` :TOPIC {Date:'2015-10-12',Keywords:'Dr Martin Boots'}) ,
  (`9` :DELIVERY {Address:'2 Hill',PostCode:'SW5'}) ,
  (`10` :IP {IP:'192.168.0.1',Platform:'IOS',Device:'Iphone',Latitude:'0.46545',Longitude:'75.151651'}) ,
  (`0`)-[:`GENERATES`]->(`1`),
  (`1`)-[:`WITH`]->(`2`),
  (`1`)-[:`WITH`]->(`3`),
  (`0`)-[:`VIEW`]->(`4`),
  (`0`)-[:`VIEW`]->(`5`),
  (`0`)-[:`RATES`]->(`6`),
  (`0`)-[:`WISH`]->(`7`),
  (`0`)-[:``SEARCH FOR``]->(`8`),
  (`0`)-[:``RECEIVE DELIVERY``]->(`9`),
  (`0`)-[:``ACCESS FROM``]->(`10`)