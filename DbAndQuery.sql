create table divisions(
    id serial primary key,
    name varchar(200) not null
);

create table wells(
    id serial primary key,
    name varchar(200) not null,
    commissioning_date timestamp not null,
    division_id int not null references divisions(id)
);

create table measurements(
    id serial primary key,
    measurement_time timestamp not null,
    well_id int not null references wells(id),
    measurement_value decimal(10, 4) not null,
    measurement_type int not null
);

select d.name as Подразделение,
       w.name as "Название скважины",
       date(m.measurement_time) as "Дата замера",
       m.measurement_type as "Тип замера",
       min(m.measurement_value) as "Минимальное значение замера",
       max(m.measurement_value) as "Максимальное значение замера",
       count(m.measurement_value) as "Количество замеров"
from divisions d
join wells w
on w.division_id = d.id
join measurements m
on m.well_id = w.id
group by date(m.measurement_time), d.name, w.name, m.measurement_type;