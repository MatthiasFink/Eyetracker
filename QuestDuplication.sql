


update slide set filepath = concat('EyeTracking/img/', cast(substring(filepath, 1, len(filepath)-4) as int) + 1, '.png')
;
go

select * from slide;

update slide set filepath = concat('E:\Visual Studio Projects\Fink\EyetrackerProject\EyetrackerExperiment\', filepath);

insert into questionnaire ([Test_Definition_Id], [Title], [Description], [Help], [Step])
select 2, [Title], [Description], [Help], [Step] from questionnaire
where test_definition_id = 1;

select * from slide;

insert into slide (test_definition_id, num, title, filepath, image_mime, duration, is_multiple_choice)
select 2, num, title, filepath, image_mime, duration, is_multiple_choice
from slide where test_definition_id = 1;

insert into slide_choice(slide_id, shortcut, num, choice)
select slide_id + 20, shortcut, num, choice from slide_choice where slide_id <= 20;

select * from question;

insert into question (questionnaire_id, type_cd, num, question, correct_answer)
select questionnaire_id + 2, type_cd, num, question, correct_answer
from Question;

select * from choice;

insert into choice(question_id, num, shortcut, choice, is_correct)
select nq.id, c.num, c.shortcut, c.choice, c.is_correct 
from choice c 
join question oq on oq.id = c.question_id
join question nq on nq.num = oq.num and nq.questionnaire_id = oq.questionnaire_id + 2;

update slide set filepath = null;