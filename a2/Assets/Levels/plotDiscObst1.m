
%plot existing discrete obstacle map
load discObst1

A=ones(11,11);
A(1,:)=zeros(1,11);
A(11,:)=zeros(1,11);
A(6,:)=zeros(1,11);
A(:,1)=zeros(11,1);
A(:,11)=zeros(11,1);    
A(5,6)=0;
A(3:7,2)=zeros(5,1);
A(3:7,10)=zeros(5,1);



spy(A,30,'s')
hold on


startPos=[
    2 3;
    2 4;
    10 5;
    10 7
    ];

goalPos=[
    10 3;
    10 4;
    2 5;
    2 7
    ];

customerPos=[
1 1;
1 11;
6 1;
6 6;
6 11;
11 11;
11 1
];



plot(startPos(:,1),startPos(:,2),'*')
plot(goalPos(:,1),goalPos(:,2),'x')
plot(customerPos(:,1),customerPos(:,2),'o');


for i=1:length(startPos)
    plot([startPos(i,1) goalPos(i,1)],[startPos(i,2) goalPos(i,2)]);
end

%creating new maps
%A=round(0.7*rand(20,20));
% startPos=[2 2];
% goalPos=[15 20];

save('discObst1','A','startPos','goalPos')

print -deps2 discreteMaze1.eps


