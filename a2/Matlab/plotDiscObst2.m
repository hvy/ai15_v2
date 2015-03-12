
%plot existing discrete obstacle map
load discObst2

% A=ones(11,11);
% A(1,:)=zeros(1,11);
% A(11,:)=zeros(1,11);
% A(6,:)=zeros(1,11);
% A(:,1)=zeros(11,1);
% A(:,11)=zeros(11,1);    
% A(5,6)=0;
% A(3:7,2)=zeros(5,1);
% A(3:7,10)=zeros(5,1);

%creating new maps
%A=round(0.8*rand(20,20));
% startPos=[2 2];
% goalPos=[15 20];

spy(A,20,'s')
hold on


startPos=[
    16 13;
    5 20;
    8 12;
    2 7];

goalPos=[
    16 20;
    17 3;
    11 6;
    14 6];

customerPos=[
2 19;
4 14;
7 9;
6 4;
12 1;
12 3;
14 9;
16 6;
17 5;
20 18
];

plot(startPos(:,1),startPos(:,2),'*')
plot(goalPos(:,1),goalPos(:,2),'x')
plot(customerPos(:,1),customerPos(:,2),'o');


for i=1:length(startPos)
    plot([startPos(i,1) goalPos(i,1)],[startPos(i,2) goalPos(i,2)]);
end


save('discObst2','A','startPos','goalPos')

print -deps2 discreteMaze2.eps


